
import { useNavigate } from "react-router-dom";
const Success = () => {
  const nav = useNavigate();
  return (
    <div style={{ textAlign: "center", marginTop: "100px" , color: "green"}}>
      <h2 style={{color: "red"}}>✅ Payment Successful</h2>
      <p style={{color: "red"}}>Your payment is being processed.</p>
      <button onClick={() => nav("/")}>Go Home</button>
    </div>
  );
};

export default Success;