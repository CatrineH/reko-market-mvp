import { useMsal } from "@azure/msal-react";
import { useCallback, useState } from "react";
import { apiRequest } from "../auth/authConfig";
import type { IAnalysisSuggestion } from "./IAnalysisSuggestion";

function useAnalyzeProductImage() {
    const { instance, accounts } = useMsal();
    const [analyzing, setAnalyzing] = useState(false);
    const [analysisError, setAnalysisError] = useState<string | null>(null);
    const [suggestion, setSuggestion] = useState<IAnalysisSuggestion | null>(null);

    const analyze = useCallback(async (file: File) => {
        setAnalyzing(true);
        setAnalysisError(null);
        setSuggestion(null);

        try {
            const { accessToken } = await instance.acquireTokenSilent({
                ...apiRequest,
                account: accounts[0],
            });

            const formData = new FormData();
            formData.append("FormFile", file);

            const response = await fetch("https://localhost:7213/api/image-processing/analyze", {
                method: "POST",
                headers: { Authorization: `Bearer ${accessToken}` },
                body: formData,
            });

            if (response.status === 201) {
                const data = await response.json();
                setSuggestion({
                    name: data.name ?? null,
                    category: data.category ?? null,
                    weight: data.weight ?? null,
                    price: data.price ?? null,
                });
            } else if (response.status === 422) {
                const data = await response.json();
                setAnalysisError(data.error ?? "Bildeanalyse feilet.");
            } else if (response.status === 400) {
                setAnalysisError("Ingen fil mottatt av serveren.");
            } else {
                throw new Error(`Unexpected response: ${response.status}`);
            }
        } catch (err) {
            if ((err as { name?: string }).name === "InteractionRequiredAuthError") {
                instance.acquireTokenRedirect({ ...apiRequest, account: accounts[0] });
            } else {
                setAnalysisError((err as Error).message);
            }
        } finally {
            setAnalyzing(false);
        }
    }, [instance, accounts]);

    return { analyze, analyzing, analysisError, suggestion };
}

export default useAnalyzeProductImage;
