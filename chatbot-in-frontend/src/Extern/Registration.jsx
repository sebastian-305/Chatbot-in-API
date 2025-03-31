import { handleLogin } from "../scripts/Chatbotin-Helper"

export function Registration() {

    return <form onSubmit={handle_registration}>
        <label htmlFor="registration-form-username">User Name: </label><input type="text" id="registration-form-username" name="username"></input>
        <label htmlFor="registration-form-email">E-Mail: </label><input type="email" id="registration-form-email" name="email"></input>
        <label htmlFor="registration-form-password">Password: </label><input type="password" id="registration-form-password" name="password"></input>
        <input type="submit" value="Register"></input>
    </form>;

    function handle_registration(event) {
        event.preventDefault();
        const request = new Request('http://localhost:5114/register', {
            method: 'POST',
            headers: {
                'Accept': 'application/json',
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                "username": event.target.username.value,
                "email": event.target.email.value,
                "password": event.target.password.value
            })
        });
        
        fetch(request).then(response => response.json()).then(text => {
            alert(text.message);
            if (text.status.toUpperCase() === "SUCCESS") {
                handleLogin(event.target.username.value, event.target.password.value);
            };
            
        });


    }

}