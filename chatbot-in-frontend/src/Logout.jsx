import { getCookie } from "./scripts/Chatbotin-Helper";
export function Logout() {
    function handle_logout(event) {
        event.preventDefault();
        const request = new Request('http://localhost:5114/logout', {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json',
                'Authorization': getCookie('session')
            }
        });
        fetch(request)
            .then(response => response.json())
            .then(text => console.log(text));

        document.cookie = `session=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/`;
        alert("Logged out");
        window.location.reload();
    }
    return <button onClick={handle_logout}>Logout</button>;
}