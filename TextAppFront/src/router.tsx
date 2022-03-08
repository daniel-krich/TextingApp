import { BrowserRouter as Router, Route, Routes } from "react-router-dom";
import { Home, Profile, Login, Inbox, Register } from './pages';
import { useGlobalStore } from "./services";

export default function ContentRouter(props: any) {
    const auth = useGlobalStore();
    return (
        <Router>
            <Routes>
                <Route path="/" element={<Home />}/>
                <Route path="/login" element={<Login />}/>
                <Route path="/register" element={<Register />}/>

                { auth.authService.isLogged && //Show routes only if user is logged-in
                    <>
                        <Route path="/inbox" element={<Inbox />}>
                            <Route path=":chatId" element={<Inbox />}/>   
                        </Route>
                        <Route path="/profile" element={<Profile />}>
                            <Route path=":profileId" element={<Profile />}/>   
                        </Route>
                    </>
                }

                <Route path="*" element={<Home />}/>
            </Routes>
        </Router>
    );
}