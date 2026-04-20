import './App.css';
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";

import AddProduct from './Components/AddProduct/AddProduct'
import Login from './Components/Login/Login'
import UserProfile from './Components/UserProfile/UserProfile'
import Home from './Components/Home/Home';

function App() {
  return (
    <Router>
      <>
        <Routes>
          <Route path="/" element={<Home/>} /> 
          <Route path="/login" element={<Login />} />
          <Route path="/profile" element={<UserProfile />} />
          <Route path="/add-product" element={<AddProduct />} />
        </Routes>

      </>
    </Router>
  )
}

export default App