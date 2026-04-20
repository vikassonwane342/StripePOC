 
const BASE_URL = "https://localhost:7270/api/payment";

export const createOrder = async (amount: number) => {
  const res = await fetch(`${BASE_URL}/create-order`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify(amount)
  });

  if (!res.ok) throw new Error("Failed to create order");

  return res.json();
};