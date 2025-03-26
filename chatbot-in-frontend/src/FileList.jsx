import { useState, useEffect } from "react";
import { getCookie } from "./scripts/Chatbotin-Helper.js"

export function FileList() {
    const [files, setFiles] = useState([]);

    useEffect(() => {
        var session_token = getCookie("session");

        const request = new Request('http://localhost:5114/files', {
            method: 'GET',
            headers: {
                "Authorization": session_token
            }
        });
        fetch(request).then(response => response.json()).then(text => { setFiles(text.map(e => e.fileName)) });
    }, [])

    return <ul>
        {files.map(create_list_element)}
    </ul>;

    function delete_file(index) {
        const filename = document.getElementById(`label-file${index}`).innerHTML;

        const request = new Request('http://localhost:5114/delete', {
            method: 'DELETE',
            headers: {
                "filename": filename,
                "Authorization": getCookie("session")
            }
        });
        fetch(request).then(response => response.text()).then(text => console.log(text));
    }

    function create_list_element(filename, index) {
        return (
            < li key={index} >
                {/*<input type="checkbox" id={"checkbox-file" + index}></input>*/}
                <label htmlFor={"file" + index} id={"label-file" + index}>{filename}</label>
                <button type="button" onClick={() => delete_file(index)}>delete</button>
            </li>
        );
    }


}