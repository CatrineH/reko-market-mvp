import { useMsal } from "@azure/msal-react";
import { useEffect, useState } from "react";
import { apiRequest } from "../auth/authConfig";
import type { IProduct } from "./IProduct";

function useFetchAllProductsFromDatabase() {
    const { instance, accounts } = useMsal();
    const [products, setProducts] = useState<IProduct[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);
    const [refetchIndex, setRefetchIndex] = useState(0);

    const refetch = () => setRefetchIndex((prev) => prev + 1);

    useEffect(() => {
        const fetchProducts = async () => {
            try {
                const { accessToken } = await instance.acquireTokenSilent({
                    ...apiRequest,
                    account: accounts[0],
                });
                const response = await fetch("https://localhost:7213/api/products", {
                    headers: { Authorization: `Bearer ${accessToken}` },
                });
                if (!response.ok) throw new Error("Failed to fetch products");
                setProducts(await response.json());
            } catch (err) {
                if ((err as { name?: string }).name === "InteractionRequiredAuthError") {
                    instance.acquireTokenRedirect({ ...apiRequest, account: accounts[0] });
                } else {
                    setError((err as Error).message);
                }
            } finally {
                setLoading(false);
            }
        };

        fetchProducts();
    }, [refetchIndex, instance, accounts]);

    return { products, loading, error, refetch };
}

export default useFetchAllProductsFromDatabase;