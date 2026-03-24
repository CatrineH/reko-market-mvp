import { useState } from "react";

function Login() {
    const [username, setUsername] = useState ("");
    const [password, setPassword] = useState (""); 

  return (
    <>
    <h1>Hello Login</h1>

    <form>
        <div>
            <label>Username</label>
            <input 
            type="text"
            value={username}
            onChange={(e) => setUsername (e.target.value)}
            />
        </div>

    <div>
        <label>Password</label>
          <input
            type="text"
            value={password}
            onChange={(e) => setPassword (e.target.value)}
         />
    </div>

    <button type="submit">Logg inn</button>

    </form>
    </>
  )
}

export default Login
