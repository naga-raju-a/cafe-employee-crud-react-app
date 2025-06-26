import React from 'react';
import { TextField } from '@mui/material';

function ReusableTextField({ label, name, value, onChange, error , required = false}) {
  return (
    <TextField
      label={label}
      name={name}
      value={value}
      onChange={onChange}
      error={Boolean(error)}
      helperText={error}
      fullWidth
      margin="normal"
      required={required} 
    />
  );
}

export default ReusableTextField;
