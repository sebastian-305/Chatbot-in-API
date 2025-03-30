import {useEffect, useState } from "react"
import { SignalRContext } from "./Intern"
import { getCookie } from "../scripts/Chatbotin-Helper.js"

export function DisplayRequestCount() {

    const [RM, setRM] = useState(0);
    const [RD, setRD] = useState(0);

    useEffect(() => {
        var session_token = getCookie("session");
        fetch(new Request('http://localhost:5114/count', {
            method: 'GET',
            headers: {
                "Authorization": session_token
            }
        }));
    }, [])
    
    SignalRContext.useSignalREffect(
        "RequestCounterChanged", // Your Event Key
        (message) => {

            setRD(message.promptsLeftDay);
            setRM(message.promptsLeftMinute);
        },
    );



    return(
    <div>
            <p>Requests left this Minute: {RM} </p>
            <p>Requests left today: {RD} </p>
        </div>);


  
}