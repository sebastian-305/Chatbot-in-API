export function Login() {



    return <form onSubmit={handleLogin}>
        <label htmlFor="login-form-username">User Name </label><input type="text" id="login-form-username" name="username"></input>
        <label htmlFor="login-form-password">Password: </label><input type="password" id="login-form-password" name="password"></input>
        <input type="submit" value="Login"></input>
    </form>;

    function handleLogin(event) {
        event.preventDefault();
        const request = new Request('http://localhost:5114/login', {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                "username": event.target.username.value,
                "password": event.target.password.value
            })
        });
        fetch(request).then(response => response.json()).then(text => {
            console.log(text);
            console.log(text.token);
            console.log(text.expiration);
            document.cookie = `session=Bearer ${text.token}; expires=${new Date(text.expiration).toUTCString()}; path=/`;

            window.location.reload();
        });
    }






}


