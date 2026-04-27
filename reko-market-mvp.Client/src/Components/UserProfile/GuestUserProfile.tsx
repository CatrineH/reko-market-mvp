import { useLocation, useNavigate } from "react-router-dom";
import './UserProfile.css';

function GuestUserProfile() {
  const location = useLocation();
  const navigate = useNavigate();
  

  const username = location.state?.username ?? "Guest";

  // We want to show some dummy products for the guest user.
  // This is just for demonstration purposes to show of styling and layout.
  // The product data should reflect the actual data structure used in the app (IProduct),
  // but instead of imageUrl, we will use images stored in the assets folder.

    const products = [
        {
            id: '1',
            name: 'Epler',
            category: 'Frukt',
            description: 'Friske og saftige epler fra Biri gård, ideelle for deilig juice.',
            imageUrl: '/mockproductimages/eple.jpg',
            weight: 1000,
            price: 30,
        },
        {
            id: '2',
            name: 'Gulrøtter',
            category: 'Grønnsaker',
            description: 'Sprø og søte gulrøtter fra Biri gård.',
            imageUrl: '/mockproductimages/gulrot.jpg',
            weight: 500,
            price: 20,
        },
        {
            id: '3',
            name: 'Kyllinglår',
            category: 'Kjøtt',
            description: 'Saftige kyllinglår fra Biri gård, perfekt for søndagsmiddagen.',
            imageUrl: '/mockproductimages/kyllinglår.jpg',
            weight: 2000,
            price: 50,
        },
        {
            id: '4',
            name: 'Tomater',
            category: 'Grønnsaker',
            description: 'Modne og smakfulle tomater fra Biri gård, perfekte for salater og sauser.',
            imageUrl: '/mockproductimages/tomat.jpg',
            weight: 500,
            price: 25,
        }
    ];

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
        <button onClick={() => navigate("/guest-add-product")}>
          Legg til produkt
        </button>
      </div>

      {/* Product List */}
      <div className="product-list">
        <h3>Mine produkter</h3>(
          <ul>
            {products.map((product) => (
              <li key={product.id}>
                {product.name}<br />
                {product.category}<br />
                {product.price} NOK<br />
                {product.weight} g<br />
                {product.description}<br />
                <img src={product.imageUrl} alt={product.name} className="product-image" />
              </li>
            ))}
          </ul>
        )
      </div>
    </div>
  );
}

export default GuestUserProfile;