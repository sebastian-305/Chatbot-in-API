export function UploadFile() {




    return <form onSubmit={upoad_file}>
        <input type="file" name="file"></input>
        <input type="submit" value="Upload" ></input>
    </form>;


    function upoad_file(event) {
        event.preventDefault();        
        

        var request = new Request('http://localhost:5114/upload', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/x-binary',
                'filename': event.target.file.name,
                'token': "asdf"
            },
            body: event.target.file
        });
        fetch(request).then(response => response.text()).then(text => console.log(text));
        
    }
}