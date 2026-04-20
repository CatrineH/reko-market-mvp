import { useNavigate } from "react-router-dom";
import "./Home.css";

function Home() {
  const navigation = useNavigate();

  return (
    <>
      <div className="home-page-background">
        <div className="home-icon" onClick={() => navigation("/Login")}>
          <img src="/reko-icon.png" alt="Reko Icon" />
        </div>
      </div>
    </>
  );
}

export default Home;
