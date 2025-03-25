import { useState, useEffect } from "react";

export function FileList() {
    const [files, setFiles] = useState(["first.txt", "second.txt"]);

    useEffect(() => {
        const request = new Request('http://localhost:5114/files', {
            method: 'GET',
            headers: { "token": "asdf" }
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
            //headers: new Headers().append("token", "asdf")
            headers: {
                "token": "asdf",
                "filename": filename
            }
        });
        fetch(request).then(response => response.text()).then(text => console.log(text));
    }

    function create_list_element(filename, index) {
        return (
            < li key={index} >
                <input type="checkbox" id={"checkbox-file" + index}></input>
                <label htmlFor={"file" + index} id={"label-file" + index}>{filename}</label>
                <button type="button" onClick={() => delete_file(index)}>delete</button>
            </li>
        );
    }

}