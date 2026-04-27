import { Route, BrowserRouter as Router, Routes } from "react-router-dom";
import './App.css';

import AddProduct from './Components/AddProduct/AddProduct';
import Home from './Components/Home/Home';
import Login from './Components/Login/Login';
import UserProfile from './Components/UserProfile/UserProfile';
import ProtectedRoute from './auth/ProtectedRoute';


function App() {
  return (
    <Router>
      <>
        <Routes>
          <Route path="/" element={<Home />} />
          <Route path="/login" element={<Login />} />
          <Route path="/profile" element={<ProtectedRoute><UserProfile /></ProtectedRoute>} />
          <Route path="/add-product" element={<ProtectedRoute><AddProduct /></ProtectedRoute>} />
        </Routes>
      </>
    </Router>
  )
}

export default App