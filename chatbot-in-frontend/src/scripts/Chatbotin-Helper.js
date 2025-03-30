//
export function getCookie(cname) {
    let name = cname + "=";
    let decodedCookie = decodeURIComponent(document.cookie);
    let ca = decodedCookie.split(';');
    for (let i = 0; i < ca.length; i++) {
        let c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

export function isLoggedIn() {
    return getCookie('session') !== "";
}


//This is the event handler that gets triggered when the logout button is clicked.
//It assumes your backend is set up to handle POST /logout and will invalidate the token.
export function handleLogout() {
    //Prevents the default browser behavior for button clicks or form submissions. 
    //Not strictly necessary here since it's not a form, but still safe.
    //event.preventDefault();
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

export function handleLogin(user, password) {
    const request = new Request('http://localhost:5114/login', {
        method: 'POST',
        headers: {
            'Accept': 'application/json',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            "username": user,
            "password": password
        })
    });
    fetch(request).then(response => response.json()).then(text => {
        if (text.token)
            document.cookie = `session=Bearer ${text.token}; expires=${new Date(text.expiration).toUTCString()}; path=/`;

        window.location.reload();
    });
}