import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import { Home, Contacts, Login } from './screens';

export default function ContentRouter(props: any) {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<Home />}/>
                <Route path="/contacts" element={<Contacts />}/>
                <Route path="/login" element={<Login />}/>
            </Routes>
        </Router>
    );
}