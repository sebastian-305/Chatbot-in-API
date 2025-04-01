# Chatbo-in-Api

This project is a steppingstone to Innovation Project "Pocket Lawyer".  It implements the possibility to upload textfiles and ask the Chatbot about it's contents. After uploading you can still chose, which files should be considered in the prompt.

You can Upload maximum 3 files with a maximum of 40000 bytes each. The Chatbot answers in the language of the prompt and the answer is nicely formatted. At least when the Chatbot feels like it. Sometime it ignores the language and the formatting. In this case, just ask the same question again.

## How to use

 First, you have to set the User Secrets in your ASP.NET project. Here is the format:

```JSON
    {
        "ftp:user": YOURFTPUSER,
        "ftp:url": YOURFTPURL,
        "ftp:password": YOURFTPPASSWORD,
        "db:connectionstring": "DataSource=YOURDATABASESERVER;DataBase=YOURDATABASE;UserID=YOURDATABASEADMINUSER;Password=YOURDATABASEADMINUSERPASSWORD",
        "Gemini:Key": YOURGEMINIAPIKEY
    }
```

You may find a User-Secrets `.json` file in the Zip-folder Aswell as the final presentation.

Then you should be able to start the Server. Start the http server (not https!).  

Second, you can start the React-Project. Node with NPM should already be installed on your computer. Just navigate in the chatbot-in-frontend server (the one with the `package.json` file) and install the modules with `npm install`. Then you can start the project with `npm run dev`.

We wish you meaningful insights!

## Technical information

The files will be transfered and saved as binary files. Although we only allow textfiles and open them as textfiles, in future Versions the API should be capable of processing differend filetypes like .pdf, .docx, etc. Therefore we transfer the files as binary data and may allow the other filetypes as soon as we have implemented the appropiate parsing/deserialization mechanism.

## Credits

(c) Alena Urbel, Sebastian Bogensperger
Link to our private repo: <https://github.com/sebastian-305/Chatbot-in-API>
