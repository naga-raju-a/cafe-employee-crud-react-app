import React from 'react';
import { Routes, Route } from 'react-router-dom';
import { Container } from '@mui/material';
import CafePage from './components/CafePage';
import EmployeePage from './components/EmployeePage';


function App() {
  return (
    <Container sx={{ mt: 4 }}>
      <Routes>
        <Route path="/" element={<CafePage />} />
        <Route path="/cafes" element={<CafePage />} />        
        <Route path="/employees" element={<EmployeePage />} />
       
      </Routes>
    </Container>
  );
}

export default App;
