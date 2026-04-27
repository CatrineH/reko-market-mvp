import { Route, BrowserRouter as Router, Routes } from "react-router-dom";
import './App.css';

import AddProduct from './Components/AddProduct/AddProduct';
import Home from './Components/Home/Home';
import Login from './Components/Login/Login';
import UserProfile from './Components/UserProfile/UserProfile';
import ProtectedRoute from './auth/ProtectedRoute';
import GuestUserProfile from "./Components/UserProfile/GuestUserProfile";
import GuestAddProduct from "./Components/AddProduct/GuestAddProduct";

function App() {
  return (
    <Router>
      <>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/login" element={<Login />} />
          <Route path="/profile" element={<ProtectedRoute><UserProfile /></ProtectedRoute>} />
          <Route path="/add-product" element={<ProtectedRoute><AddProduct /></ProtectedRoute>} />
          <Route path="/guest-profile" element={<GuestUserProfile />} />
          <Route path="/guest-add-product" element={<GuestAddProduct />} />
        </Routes>
      </>
    </Router>
  )
}

export default App