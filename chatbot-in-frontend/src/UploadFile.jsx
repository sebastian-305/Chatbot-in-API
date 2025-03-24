export function UploadFile() {
    return <form onSubmit={upoad_file}>
        <input type="file" name="myfile"></input>
        <input type="submit" value="Upload" ></input>
    </form>;


    function upoad_file(event) {
        event.preventDefault();
        console.log(event.target.myfile.files[0]);

        var request = new Request('http://localhost:5114/upload', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-binary',
                'filename': event.target.myfile.files[0].name,
                'token': "asdf"
            },
            body: event.target.myfile.files[0]
        });
        fetch(request).then(response => response.status).then(text => {
            console.log("Response:");
            console.log(text);
        });

    }
}