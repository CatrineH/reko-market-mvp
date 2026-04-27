import { useState, useEffect } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import './AddProduct.css';
import useAddProductToDatabase from '../../Hooks/useAddProductToDb';
import useAnalyzeProductImage from '../../Hooks/useAnalyzeProductImage';


function AddProduct() {
  const [productName, setProductName] = useState('');
  const [productCategory, setProductCategory] = useState('');
  const [productDescription, setProductDescription] = useState('');
  const [productPrice, setProductPrice] = useState('');
  const [productWeight, setProductWeight] = useState('');
  const [productImage, setProductImage] = useState<File | null>(null);

  const { submit, loading, error, validationErrors, createdProduct } = useAddProductToDatabase();
  const { analyze, analyzing, analysisError, suggestion } = useAnalyzeProductImage();
  const navigate = useNavigate();

  useEffect(() => {
    if (createdProduct) {
      navigate('/profile');
    }
  }, [createdProduct, navigate]);

  useEffect(() => {
    if (!suggestion) return;
    if (suggestion.name) setProductName(suggestion.name);
    if (suggestion.category) setProductCategory(suggestion.category);
    if (suggestion.weight) setProductWeight(String(suggestion.weight));
    if (suggestion.price) setProductPrice(String(suggestion.price));
  }, [suggestion]);

  const handleSubmit = async (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    if (!productImage) return;

    await submit({
      name: productName,
      category: productCategory,
      description: productDescription,
      formFile: productImage,
      weight: Number(productWeight),
      price: Number(productPrice),
    });
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
          {validationErrors.name && <span className="field-error">{validationErrors.name[0]}</span>}
        </div>

        <div className="product-name-input">
          <label>Kategori</label>
          <input
            type="text"
            value={productCategory}
            onChange={(e) => setProductCategory(e.target.value)}
          />
          {validationErrors.category && <span className="field-error">{validationErrors.category[0]}</span>}
        </div>

        <div className="product-description-input">
          <label>Beskrivelse</label>
          <textarea
            value={productDescription}
            onChange={(e) => setProductDescription(e.target.value)}
          />
          {validationErrors.description && <span className="field-error">{validationErrors.description[0]}</span>}
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
          {validationErrors.price && <span className="field-error">{validationErrors.price[0]}</span>}
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
          {validationErrors.weight && <span className="field-error">{validationErrors.weight[0]}</span>}
        </div>

        <div className="product-image-input">
          <label>Bilde</label>
          <input
            type="file"
            accept="image/*"
            onChange={(e) => setProductImage(e.target.files?.[0] ?? null)}
          />
          <button
            type="button"
            onClick={() => productImage && analyze(productImage)}
            disabled={!productImage || analyzing}
          >
            {analyzing ? 'Analyserer...' : 'Analyser bilde'}
          </button>
          {analysisError && <span className="field-error">{analysisError}</span>}
          {validationErrors.formFile && <span className="field-error">{validationErrors.formFile[0]}</span>}
        </div>

        {error && <p className="submit-error">{error}</p>}

        <button className="confirm-product-button" type="submit" disabled={loading}>
          {loading ? 'Lagrer...' : 'Neste'}
        </button>

      </form>
    </div>
  );
}

export default AddProduct;
