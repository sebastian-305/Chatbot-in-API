import { handleLogin } from "../scripts/Chatbotin-Helper"

export function Login() {



    return <form onSubmit={beforeLogin}>
        <label htmlFor="login-form-username">User Name </label><input type="text" id="login-form-username" name="username"></input>
        <label htmlFor="login-form-password">Password: </label><input type="password" id="login-form-password" name="password"></input>
        <input type="submit" value="Login"></input>
    </form>;

    function beforeLogin(event) {
        event.preventDefault();

        handleLogin(event.target.username.value, event.target.password.value);
    }






}


