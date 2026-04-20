import { useEffect, useState } from "react";
import { loadStripe } from "@stripe/stripe-js";
import type { Stripe } from "@stripe/stripe-js";
import { Elements } from "@stripe/react-stripe-js";
import StripeForm from "../components/StripeForm";
import { createOrder } from "../services/stripeService";

const Checkout = () => {
  const [clientSecret, setClientSecret] = useState("");
  const [stripePromise, setStripePromise] =
    useState<Promise<Stripe | null> | null>(null);
 

  return (
    <>
      <button
        onClick={() => {
          createOrder(500).then((res) => {
            setClientSecret(res.clientSecret);
            setStripePromise(loadStripe(res.publishableKey));
          });
        }}
      >
        {" "}
        Go Payment
      </button>
      <Elements stripe={stripePromise} options={{ clientSecret }}>
        <StripeForm />
      </Elements>
    </>
  );
};

export default Checkout;
