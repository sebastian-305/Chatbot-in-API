import { useState } from 'react'
import './App.css'

function App() {
  const [count, setCount] = useState(0)

  return (
    <>
      <h1>Mockup Frontend</h1>
      <p className="read-the-docs">
        //Todo
          </p>

          <button onClick={getMockAnswer}>Get Token</button>
          <button onClick={getMockAnswer2}>Upload File</button>
          <button onClick={getMockAnswer3}>Get Files</button>
          <button onClick={getMockAnswer4}>Chat</button>
          <div id="answer"></div>

    </>
  )

    function getMockAnswer() {
        var request = new Request('http://localhost:5114/token', {
            method: 'GET'  
        });        
        fetch(request).then(response => response.text()).then(text => document.querySelector("#answer").innerHTML = text);        
    }

    function getMockAnswer2() {
        var request = new Request('http://localhost:5114/upload', {
            method: 'POST'
        });
        fetch(request).then(response => response.text()).then(text => document.querySelector("#answer").innerHTML = text);
    }


    function getMockAnswer3() {
        var request = new Request('http://localhost:5114/files', {
            method: 'GET'
        });
        fetch(request).then(response => response.text()).then(text => document.querySelector("#answer").innerHTML = text);
    }

    function getMockAnswer4() {
        var request = new Request('http://localhost:5114/chat', {
            method: 'POST'
        });
        fetch(request).then(response => response.text()).then(text => document.querySelector("#answer").innerHTML = text);
    }
}

export default App
