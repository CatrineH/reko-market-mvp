import { useMsal } from "@azure/msal-react";
import { useCallback, useState } from "react";
import { apiRequest } from "../auth/authConfig";

function useDeleteProductFromDatabase() {
    const { instance, accounts } = useMsal();
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [notFound, setNotFound] = useState(false);

    const deleteProduct = useCallback(async (id: string) => {
        setLoading(true);
        setError(null);
        setNotFound(false);

        try {
            const { accessToken } = await instance.acquireTokenSilent({
                ...apiRequest,
                account: accounts[0],
            });

            const response = await fetch(`https://localhost:7213/api/products/${id}`, {
                method: "DELETE",
                headers: { Authorization: `Bearer ${accessToken}` },
            });

            if (response.status === 404) {
                setNotFound(true);
            } else if (response.status !== 204) {
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

    return { deleteProduct, loading, error, notFound };
}

export default useDeleteProductFromDatabase;
