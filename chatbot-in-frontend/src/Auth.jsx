import ChatbotinContext from "./ChatbotinContext"
import { Registration } from "./Extern/Registration"
import { Login } from "./Extern/Login"
import { useEffect, useContext } from "react";
import { isLoggedIn } from "./scripts/Chatbotin-Helper";
import { Intern } from "./Intern/Intern"


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