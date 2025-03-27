//This line imports a helper function called getCookie from a file in your scripts folder.

//getCookie('session') is used to retrieve the session token saved in a browser cookie.

//This token is needed for authorization when making secure API requests.

import { getCookie } from "./scripts/Chatbotin-Helper";
export function Logout() {

    //This is the event handler that gets triggered when the logout button is clicked.
    //It assumes your backend is set up to handle POST /logout and will invalidate the token.
    function handle_logout(event) {
        //Prevents the default browser behavior for button clicks or form submissions. 
        //Not strictly necessary here since it's not a form, but still safe.
        event.preventDefault();
        //Creates a POST request to the backend endpoint /logout with the Authorization header set to your session token.
        const request = new Request('http://localhost:5114/logout', {
            method: 'POST',
            headers: {
                'Accept': 'application/json', //sending/expecting JSON
                'Content-Type': 'application/json', //You’re telling the server your request body is in JSON format (even though here there’s no body).
                'Authorization': getCookie('session') // Adds the Bearer <token> from the cookie to authenticate the user.
            }
        });
        fetch(request) //Sends the logout request to the server using the fetch API
            .then(response => response.json()) //Processes the JSON response from the backend.
            .then(text => console.log(text)); //Logs the response (e.g. "Logout successful") to the console for debugging.

        document.cookie = `session=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/`; //This deletes the session cookie on the client side by setting an expiration date in the past.
        alert("Logged out"); //Displays a popup message to the user for confirmation.
        window.location.reload();//Reloads the whole page so your app reflects the logout
    } 
    return <button onClick={handle_logout}>Logout</button>; //This is the button that triggers the logout event handler.
}