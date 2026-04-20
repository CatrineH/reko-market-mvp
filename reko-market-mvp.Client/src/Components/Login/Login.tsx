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
          <div className="usernameInput">
            <label>Username</label>
            <input
              type="text"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
            />
          </div>

          <div className="passwordInput">
            <label>Password</label>
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
          </div>

          <button className="login-button" type="submit">
            Logg inn
          </button>
        </form>
      </div>
    </>
  );
}

export default Login;
