import { useEffect, useState } from "react"
import {SignalRContext } from "./Intern"

export function DisplayRequestCount() {

    const [RM, setRM] = useState(0);
    const [RD, setRD] = useState(0);

    const [messages, setMessage] = useState([]);

    SignalRContext.useSignalREffect(
        "counter", // Your Event Key
        (message) => {
            setMessage([ message]);
        },
    );


    useEffect(() => {
        console.log(messages)
    },
     [messages]
    )


    return(
    <div>
            <p>Requests left this Minute: {RM} </p>
            <p>Requests left today: {RD} </p>
        </div>);


  
}