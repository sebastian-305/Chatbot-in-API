import { useContext } from "react"
import { getCookie } from "../scripts/Chatbotin-Helper";
import ChatbotinContext from "../ChatbotinContext";

export function UploadFile() {
    const {filesChanged, setFilesChanged } = useContext(ChatbotinContext);

    return <form onSubmit={upoad_file}>
        <input type="file" name="myfile"></input>
        <input type="submit" value="Upload" ></input>
    </form>;


    function upoad_file(event) {
        event.preventDefault();

        var request = new Request('http://localhost:5114/upload', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-binary',
                'filename': event.target.myfile.files[0].name,
                'Authorization': getCookie('session')
            },
            body: event.target.myfile.files[0]
        });
        fetch(request).then(response => {
            let status = response.status
            console.log("Response:");
            console.log(status);
            if (status === 202) {
                setFilesChanged(filesChanged + 1);
                event.target.myfile.value = null;
                //alert("File Upload completed");
            }
            else {
                response.text().then(message => alert(message));
            }
        });

    }
}