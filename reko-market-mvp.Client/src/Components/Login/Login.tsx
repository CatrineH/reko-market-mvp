import { useIsAuthenticated, useMsal } from "@azure/msal-react";
import { useState } from "react";
import { Navigate, useNavigate } from "react-router-dom";
import msLogo from "../../assets/ms-symbollockup_mssymbol_19.svg";
import { apiRequest } from "../../auth/authConfig";
import "./Login.css";


function Login() {
  // -- MS LOGIN hooks --
  const { instance } = useMsal();
  const isAuthenticated = useIsAuthenticated();
  // -- MS LOGIN hooks end --

  // -- MOCK LOGIN hooks --
  // TODO: Remove these when MS login is fully implemented
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const navigate = useNavigate();
  // -- MOCK LOGIN hooks end --

  // -- MS LOGIN start --
  if (isAuthenticated) {
    return <Navigate to="/profile" />;
  }

  const handleLoginWithMicrosoft = () => {
    instance.loginRedirect(apiRequest);
  };
  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();

    console.log("Submit triggered");
    console.log(`Velkommen, ${username}`);

    navigate("/guest-profile", { state: { username } });
  };
  // -- MOCK LOGIN end --

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
          <br />
          <button className="login-with-ms-button" onClick={handleLoginWithMicrosoft}>
            <img src={msLogo} alt="Microsoft" />
            Logg inn med Microsoft
          </button>
        </form>
      </div>
    </>
  );
}

export default Login;
