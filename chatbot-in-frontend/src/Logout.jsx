//This line imports a helper function called getCookie from a file in your scripts folder.

import { handleLogout } from "./scripts/Chatbotin-Helper";

//getCookie('session') is used to retrieve the session token saved in a browser cookie.

//This token is needed for authorization when making secure API requests.


export function Logout() {       
    return <button onClick={handleLogout}>Logout</button>; //This is the button that triggers the logout event handler.
}