import { useMsal } from "@azure/msal-react";
import { useCallback, useState } from "react";
import { apiRequest } from "../auth/authConfig";
import type { IProduct } from "./IProduct";
import type { IProductFormData } from "./IProductFormData";

function useAddProductToDatabase() {
    const { instance, accounts } = useMsal();
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [validationErrors, setValidationErrors] = useState<Record<string, string[]>>({});
    const [createdProduct, setCreatedProduct] = useState<IProduct | null>(null);

    const submit = useCallback(async (data: IProductFormData) => {
        setLoading(true);
        setError(null);
        setValidationErrors({});
        setCreatedProduct(null);

        try {
            const { accessToken } = await instance.acquireTokenSilent({
                ...apiRequest,
                account: accounts[0],
            });

            const formData = new FormData();
            formData.append("Name", data.name);
            formData.append("Category", data.category);
            formData.append("Description", data.description);
            formData.append("FormFile", data.formFile);
            formData.append("Weight", String(data.weight));
            formData.append("Price", String(data.price));

            const response = await fetch("https://localhost:7213/api/products", {
                method: "POST",
                headers: { Authorization: `Bearer ${accessToken}` },
                body: formData,
            });

            if (response.status === 201) {
                setCreatedProduct(await response.json());
            } else if (response.status === 400) {
                const problem = await response.json();
                setValidationErrors(problem.errors ?? {});
            } else {
                throw new Error(`Unexpected response: ${response.status}`);
            }
        } catch (err) {
            if ((err as { name?: string }).name === "InteractionRequiredAuthError") {
                instance.acquireTokenRedirect({ ...apiRequest, account: accounts[0] });
            } else {
                setError((err as Error).message);
            }
        } finally {
            setLoading(false);
        }
    }, [instance, accounts]);

    return { submit, loading, error, validationErrors, createdProduct };
}

export default useAddProductToDatabase;
