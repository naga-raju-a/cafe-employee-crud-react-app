import React, { useEffect, useState } from 'react';
import { useLocation, useNavigate } from 'react-router-dom';
import { Box, Tabs, Tab } from '@mui/material';

function NavigationTabs() {
  const location = useLocation();
  const navigate = useNavigate();
  const [value, setValue] = useState(0); // 0 = Cafes, 1 = Employees

  useEffect(() => {
    if (location.pathname.includes('/employees')) {
      setValue(1);
    } else {
      setValue(0); // default to Cafes
    }
  }, [location.pathname]);

  const handleChange = (event, newValue) => {
    setValue(newValue);
    if (newValue === 0) navigate('/cafes');
    else if (newValue === 1) navigate('/employees');
  };

  return (
    <Box sx={{ width: '100%', typography: 'body1', mb: 3 }}>
      <Tabs value={value} onChange={handleChange} aria-label="navigation tabs">
        <Tab label="Cafes" />
        <Tab label="Employees" />
      </Tabs>
    </Box>
  );
}

export default NavigationTabs;
