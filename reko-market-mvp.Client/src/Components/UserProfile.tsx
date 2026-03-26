import { useLocation, useNavigate } from "react-router-dom";

function UserProfile() {
  const location = useLocation();
  const navigate = useNavigate();

  const username = location.state?.username || "Guest";

  // Temporary product list
  const products = [
    { id: 1, name: "Melk", price: "20 NOK" },
    { id: 2, name: "Egg", price: "35 NOK" },
    { id: 3, name: "Poteter", price: "15 NOK" },
  ];

  return (
    <div className="profile-container">

      {/* Profile Header */}
      <div className="profile-header">
        <div className="profile-image"></div>

        <div className="profile-info">
          <h2 className="profile-username">{username}</h2>
          <p className="profile-detail">Farm: Biri gård</p>
          <p className="profile-detail">Location: Norway</p>
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

        <ul>
          {products.map((product) => (
            <li key={product.id}>
              {product.name} - {product.price}
            </li>
          ))}
        </ul>
      </div>

    </div>
  );
}

export default UserProfile;