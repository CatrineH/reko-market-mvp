import './App.css'
import { BrowserRouter as Router, Routes, Route } from "react-router-dom";

import AddProduct from './Components/AddProduct'
import Login from './Components/Login'
import UserProfile from './Components/UserProfile'

function App() {
  return (
    <Router>
      <>
        <h1>Hello Reko</h1>

        <Routes>
          <Route path="/" element={<Login />} />
          <Route path="/profile" element={<UserProfile />} />
          <Route path="/add-product" element={<AddProduct />} />
        </Routes>

      </>
    </Router>
  )
}

export default App