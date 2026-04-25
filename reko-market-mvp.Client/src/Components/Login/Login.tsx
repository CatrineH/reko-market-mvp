import "./Login.css";
import { useState } from "react";
import { useNavigate } from "react-router-dom";

function Login() {
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const navigate = useNavigate();
  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    console.log("Submit triggered");
    console.log(`Velkommen, ${username}`);

    navigate("/profile", { state: { username } });
  };

  return (
    <>
    
      <div className="login-form">
        <form onSubmit={handleSubmit}>

          <h1>Logg inn</h1>
          
          <div className="usernameInput">
            <label>Navn eller e-post</label>
            <input
              type="text"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
            />
          </div>

          <div className="passwordInput">
            <label>Passord</label>
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
          </div>

          <a href="#" className="register-link">
            Registrer deg
          </a>

          <button className="login-button" type="submit">
            Logg inn
          </button>

          <a href="#" className="forgot-password-link">
            Glemt passord?
          </a>

        </form>
      </div>
    </>
  );
}

export default Login;
