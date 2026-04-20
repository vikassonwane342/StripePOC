import { useNavigate } from "react-router-dom";
import {
  PaymentElement,
  useStripe,
  useElements,
} from "@stripe/react-stripe-js";

const StripeForm = () => {
  const stripe = useStripe();
  const elements = useElements();

  const handleSubmit = 
   async (e: React.FormEvent) => {
    e.preventDefault();

    if (!stripe || !elements) return;

    const result = await stripe.confirmPayment({
      elements,
      confirmParams: {
        return_url: "http://localhost:5173/success",
      },
    });

    if (result.error) {
      console.error(result.error.message);
      alert("Payment Failed");
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <PaymentElement />

      <button type="submit" disabled={!stripe} className="btn btn-danger">
        Pay ₹500
      </button>
    </form>
  );
};

export default StripeForm;
