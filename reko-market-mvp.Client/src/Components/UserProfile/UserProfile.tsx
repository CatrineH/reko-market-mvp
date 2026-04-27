import { useAccount, useMsal } from "@azure/msal-react";
import { useLocation, useNavigate } from "react-router-dom";
import useFetchAllProductsFromDatabase from "../../Hooks/useFetchAllProductsFromDb";
import "./UserProfile.css";

function UserProfile() {
  const location = useLocation();
  const navigate = useNavigate();
  const { instance, accounts } = useMsal();
  const account = useAccount(accounts[0]);

  const { products, loading, error, refetch } =
    useFetchAllProductsFromDatabase();

  const username = account?.name ?? location.state?.username ?? "Guest";

  const handleLogout = () => {
    instance.logoutRedirect();
  };

  return (
    <div className="profile-container">
      {/* Profile Header */}
      <div className="profile-header">
        <div className="profile-img">ProfilBilde</div>
        <div className="profile-info">
          <h2 className="profile-username">{username}</h2>
          <p className="profile-role">Produsent Gjøvik</p>
          <p className="profile-phone">+47 999 99 999</p>
        </div>
      </div>

      <div className="farm-info">
        {/* Farm Card */}
        <div className="farm-card">
          <div className="farm-img"></div>
          <h3>Biri Gård</h3>
          <p>
            På gården drives det produksjon av egg og honning, samt holdes det
            flere dyr på gården.
          </p>
          <button className="farm-more-btn"> Les mer </button>

          {/* Action Buttons */}
          <div className="action-buttons">
            <button
              className="add-btn"
              onClick={() => navigate("/add-product")}
            >
              Legg til nytt produkt
            </button>

            <button className="secondary-btn">
              Oversikt over bestillinger
            </button>
          </div>
        </div>
        {/* Product List */}
        <div className="product-list">
          <h3>Mine produkter</h3>

          {products.map((product) => (
            <div className="product-card" key={product.id}>
              <img src={product.imageUrl} alt={product.name} />

              <div className="product-info">
                <p>{product.category}</p>
                <h4>{product.name}</h4>
                <p>{product.description}</p>
                <p>{product.price}</p>
              </div>

              <button className="edit-btn">Redigere</button>
            </div>
          ))}
        </div>
      </div>

      {/* Bottom Navigation */}
      <div className="bottom-nav"></div>
      <button onClick={handleLogout}>Logg ut</button>
    </div>
  );
}

export default UserProfile;
