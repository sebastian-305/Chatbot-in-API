import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'

function App() {
  const [count, setCount] = useState(0)

  return (
    <>
      <h1>Mockup Frontend</h1>
      <p className="read-the-docs">
        //Todo
          </p>

          <button onClick={getMockAnswer }>Send request</button>
          <div id="answer"></div>

    </>
  )

    function getMockAnswer() {
        var request = new Request('http://localhost:5114/token', {
            method: 'GET'  
        });

        fetch(request)
            .then((response) => response.json())
            .then((responseJson) => {
                document.querySelector("#answer").innerHTML = responseJson;
            })
            .catch((error) => {
                console.error(error);
            });

    }
}

export default App
