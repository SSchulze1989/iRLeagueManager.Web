// Bridges Blazor Server components to the browser's fetch API for the cookie based
// "/api/auth/*" endpoints. This must run in the browser (rather than as a server-side
// HttpClient call from the Blazor circuit) because the HttpOnly "X-Access-Token" cookie
// set by these endpoints has to be received and stored by the user's browser.
window.authInterop = {
    postJson: async function (url, payload) {
        try {
            const response = await fetch(url, {
                method: 'POST',
                credentials: 'same-origin',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(payload ?? {})
            });

            let body = null;
            try {
                body = await response.json();
            } catch {
                body = null;
            }

            return {
                success: response.ok,
                status: response.status,
                body: body
            };
        } catch (error) {
            return {
                success: false,
                status: 0,
                body: null
            };
        }
    }
};
