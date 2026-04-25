import { useIsAuthenticated } from "@azure/msal-react";
import { Navigate } from "react-router-dom";

function ProtectedRoute({ children }: { children: React.ReactNode }) {
    const isAuthenticated = useIsAuthenticated();
    return isAuthenticated ? children : <Navigate to="/login" />;
}

export default ProtectedRoute;