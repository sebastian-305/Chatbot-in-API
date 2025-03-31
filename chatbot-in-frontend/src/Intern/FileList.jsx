import { useState, useEffect, useContext } from "react";
import { getCookie } from "../scripts/Chatbotin-Helper.js"
import ChatbotinContext from "../ChatbotinContext"
export function FileList() {
    const [files, setFiles] = useState([]);
    const { filesChanged, setFilesChanged, selectedFiles, addFile, removeFile } = useContext(ChatbotinContext);

    useEffect(() => {
        var session_token = getCookie("session");

        const request = new Request('http://localhost:5114/files', {
            method: 'GET',
            headers: {
                "Authorization": session_token
            }
        });
        fetch(request).then(response => response.json()).then(text => { setFiles(text.map(e => e.fileName)) });
    }, [filesChanged])

    useEffect(() => {
        document.getElementById("selected-files").innerHTML = "Selected files: " + selectedFiles.join(", ");

    }, [selectedFiles]);

    return <ul className="file-list">
        {files.map(create_list_element)}
        <li id="selected-files">Selected files:</li>
    </ul>;

    function delete_file(filename) {
        const request = new Request('http://localhost:5114/delete', {
            method: 'DELETE',
            headers: {
                "filename": filename,
                "Authorization": getCookie("session")
            }
        });
        fetch(request).then(response => response.text()).then(() =>
            setFilesChanged(filesChanged + 1)
        );
    }

    function create_list_element(filename, index) {
        return (
            < li key={index} >
                <label htmlFor={"file" + index} id={"label-file" + index}>
                    <input type="checkbox" id={"checkbox-file" + index} onChange={(e) => add_or_remove(e, { filename })}></input>

                    {filename}</label>
                <button type="button" onClick={() => delete_file(filename)}>delete</button>
            </li>
        );
    }

    function add_or_remove(event, filename) {

        if (event.target.checked) {
            addFile(filename.filename);
        }
        else {
            removeFile(filename.filename);
        }
    }
}