import { useAccount, useMsal } from "@azure/msal-react";
import { useLocation, useNavigate } from "react-router-dom";
import useFetchAllProductsFromDatabase from "../../Hooks/useFetchAllProductsFromDb";
import './UserProfile.css';
import useDeleteProductFromDatabase from "../../Hooks/useDeleteProductFromDb";

function UserProfile() {
  // -- Router hooks --
  const location = useLocation();
  const navigate = useNavigate();

  // -- Auth hooks --
  const { instance, accounts } = useMsal();
  const account = useAccount(accounts[0]);
  const username = account?.name ?? location.state?.username ?? "Guest";
  const handleLogout = () => {
    instance.logoutRedirect();
  };

  // -- Product hooks --
  const { products, loading, error, refetch } = useFetchAllProductsFromDatabase();
  const { deleteProduct, loading: deleteLoading, error: deleteError, notFound } = useDeleteProductFromDatabase();
  const handleDelete = async (id: string) => {
    await deleteProduct(id);
    refetch();
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
        {deleteError && <p>Slettefeil: {deleteError}</p>}
        {notFound && <p>Produktet finnes ikke lenger.</p>}
        {!loading && !error && (
          <ul>
            {products.map((product) => (
              <li key={product.id}>
                <hr />
                {product.name} - ( {product.category} )<br />
                {product.price} NOK<br />
                {product.weight} g<br />
                {product.description}<br />
                {product.imageUrl && <img src={product.imageUrl} alt={product.name} className="product-image" />}
                {!product.imageUrl && <div className="product-image-placeholder">[ * MANGLER BILDE * ]</div>}<br />
                <button onClick={() => handleDelete(product.id)} disabled={deleteLoading}>Slett</button>
                <br />
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