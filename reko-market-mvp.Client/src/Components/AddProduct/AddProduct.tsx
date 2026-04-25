import { useState } from 'react';
import { Link } from 'react-router-dom';
import './AddProduct.css';


function AddProduct() {
  const [productName, setProductName] = useState('');
  const [productCategory, setProductCategory] = useState('');
  const [productPrice, setProductPrice] = useState('');
  const [productWeight, setProductWeight] = useState('');

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    console.log('Legg til produkt:', productName);
    // TODO: send the product to the API or update state here
  };

  return (
    <div className="add-product-form">
      <form onSubmit={handleSubmit}>

        <Link to="/profile" className="back-link">
          &larr; Tilbake
        </Link>

        <h1>Legg til produkt</h1>

        <div className="product-name-input">
          <label>Navn</label>
          <input
            type="text"
            value={productName}
            onChange={(e) => setProductName(e.target.value)}
          />
        </div>

        <div className="product-name-input">
          <label>Kategori</label>
          <input
            type="text"
            value={productCategory}
            onChange={(e) => setProductCategory(e.target.value)}
          />
        </div>

        <div className="product-price-input">
          <label>Pris</label>
          <div className="input-with-unit">
            <input
              type="number"
              value={productPrice}
              onChange={(e) => setProductPrice(e.target.value)}
            />
            <span className="unit-label">nok</span>
          </div>
        </div>

        <div className="product-weight-input">
          <label>Vekt</label>
          <div className="input-with-unit">
            <input
              type="number"
              value={productWeight}
              onChange={(e) => setProductWeight(e.target.value)}
            />
            <span className="unit-label">gr</span>
          </div>
        </div>

        <button className="confirm-product-button" type="submit">
          Neste 
        </button>

      </form>
    </div>
  );
}

export default AddProduct;
