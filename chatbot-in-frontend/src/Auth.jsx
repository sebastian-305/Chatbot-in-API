import ChatbotinContext from "./ChatbotinContext"
import { Registration } from "./Registration"
import { Login } from "./Login"
import { useEffect, useContext } from "react";
import { isLoggedIn } from "./scripts/Chatbotin-Helper";
import { Intern } from "./Intern"


export function Auth() {
    const { loggedIn, setLoggedIn } = useContext(ChatbotinContext);


    useEffect(() => {
        setLoggedIn(isLoggedIn());
    }, []);

    return <>
        {!loggedIn && (
            <>
                <Registration></Registration>
                <Login></Login>
            </>
        )}
        {loggedIn && (
            <Intern></Intern>

        )}</>
}