import type { Configuration, RedirectRequest } from "@azure/msal-browser";

const tenantId = "470f4334-a9c7-481b-99d3-734444df6a7a";
const spaClientId = "01b746ca-f935-49d9-bee9-c0757d05a7f5";
const apiClientId = "e0db2381-2dd8-4c09-98ec-1487e8c9f8fe";

export const msalConfig: Configuration = {
    auth: {
        clientId: spaClientId,
        authority: `https://login.microsoftonline.com/${tenantId}`,
        redirectUri: "https://localhost:5173",
    },
};

export const apiRequest: RedirectRequest = {
    scopes: [`api://${apiClientId}/.default`],
};
