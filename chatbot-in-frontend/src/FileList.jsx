import { useState, useEffect } from "react";

export function FileList() {
    const [files, setFiles] = useState(["first.txt", "second.txt"]);

    useEffect(() => {
        const request = new Request('http://localhost:5114/files', {
            method: 'GET',
            //headers: new Headers().append("token", "asdf")
            headers: {"token": "asdf"}
        });
        fetch(request).then(response => response.json()).then(text => {setFiles(text.map(e=>e.fileName)) });
    }, [])
    
        
    

    return <ul>
        {files.map((file, index) => (<li key={index} ><input type="checkbox" id={"file" + index}></input><label htmlFor={"file" + index}>{file}</label> <button type="button">delete</button></li>))}</ul>;

 }