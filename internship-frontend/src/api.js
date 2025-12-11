

const API_BASE_URL = "https://localhost:7027";

export async function getInternships() {
  const response = await fetch(`${API_BASE_URL}/api/Internships`);

  if (!response.ok) {
    throw new Error("Greška prilikom dohvaćanja.");
  }

  return response.json();
}
export async function login(username, password) {
  const response = await fetch(`${API_BASE_URL}/api/Auth/login`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify({ username, password }),
  });

  if (response.status === 400) {
    const text = await response.text();
    throw new Error(text || "Neispravan zahtjev.");
  }

  if (response.status === 401) {
    const text = await response.text();
    throw new Error(text || "Neispravni podaci za prijavu.");
  }

  if (!response.ok) {
    throw new Error("Greška na serveru prilikom prijave.");
  }

  
  return response.json();
}

//dohvacanje kategorija
export async function getCategories() {
  const response = await fetch(`${API_BASE_URL}/api/Categories`);

  if (!response.ok) {
    throw new Error("Greška prilikom dohvaćanja kategorija.");
  }

  return response.json();
}

//dohvacanje tvrtka
export async function getCompanies() {
  const response = await fetch(`${API_BASE_URL}/api/Companies`);

  if (!response.ok) {
    throw new Error("Greška prilikom dohvaćanja tvrtke.");
  }

  return response.json();
}

//kreiranje internshipa
export async function createInternship(internship, token) {
  const response = await fetch(`${API_BASE_URL}/api/Internships`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify(internship),
  });

  if (response.status === 401) {
    throw new Error("Nisi autoriziran. Pokušaj se ponovo prijaviti.");
  }

  if (!response.ok) {
    const text = await response.text();
    throw new Error(text || "Greška prilikom kreiranja prakse.");
  }

  return response.json();
}