import { useAccount, useMsal } from "@azure/msal-react";
import { useLocation, useNavigate } from "react-router-dom";
import useFetchAllProductsFromDatabase from "../../Hooks/useFetchAllProductsFromDb";
import './UserProfile.css';

function UserProfile() {
  const location = useLocation();
  const navigate = useNavigate();
  const { instance, accounts } = useMsal();
  const account = useAccount(accounts[0]);
  const { products, loading, error, refetch } = useFetchAllProductsFromDatabase();

  const username = account?.name ?? location.state?.username ?? "Guest";

  const handleLogout = () => {
    instance.logoutRedirect();
  };

  return (
    <div className="profile-container">

      {/* Profile Header */}
      <div className="profile-header">
        <div className="profile-image"></div>

        <div className="profile-info">
          <h2 className="profile-username">{username}</h2>
          <p className="profile-detail">Biri gård</p>
          <p className="profile-detail">REKO-ringen Gjøvik</p>
        </div>
      </div>

      {/* Add Product Button */}
      <div className="add-product-section">
        <button onClick={() => navigate("/add-product")}>
          Legg til produkt
        </button>
      </div>

      {/* Product List */}
      <div className="product-list">
        <h3>Mine produkter</h3>
        {loading && <p>Laster produkter...</p>}
        {error && <p>Feil: {error}</p>}
        {!loading && !error && (
          <ul>
            {products.map((product) => (
              <li key={product.id}>
                {product.name} - {product.price} NOK
              </li>
            ))}
          </ul>
        )}
      </div>
      <button onClick={handleLogout}>Logg ut</button>
    </div>
  );
}

export default UserProfile;