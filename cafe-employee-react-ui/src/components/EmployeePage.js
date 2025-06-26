import React, { useCallback, useEffect, useState } from 'react';
import {
  Box, Button, Typography, IconButton, Tooltip,
  Snackbar, Alert, TextField
} from '@mui/material';
import EditIcon from '@mui/icons-material/Edit';
import DeleteIcon from '@mui/icons-material/Delete';
import { AgGridReact } from 'ag-grid-react';
import { useLocation } from 'react-router-dom';

import NavigationTabs from './common/NavigationTabs';
import ConfirmDialog from './common/ConfirmDialog';
import EmployeeDialog from './common/EmployeeDialog';
import { getEmployees, deleteEmployee } from '../services/api';

import 'ag-grid-community/styles/ag-grid.css';
import 'ag-grid-community/styles/ag-theme-material.css';

function EmployeePage() {
  // State variables for employees, dailog modals, snackbar and search
  const [employees, setEmployees] = useState([]);
  const [confirmDialog, setConfirmDialog] = useState(false);
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const [selectedEmployee, setSelectedEmployee] = useState(null);
  const [snackbar, setSnackbar] = useState({ open: false, message: '', severity: 'success' });
  const [searchValue, setSearchValue] = useState('');

  const location = useLocation();
  const queryParams = new URLSearchParams(location.search);
  const cafeId = queryParams.get('cafeId'); // Optional query filter by cafeId

  // Fetch employee data (optionally filtered by cafeId)
  const fetchEmployees = useCallback(async () => {
    try {
      const response = await getEmployees();
      const allEmployees = response.data;
      const filtered = cafeId
        ? allEmployees.filter(emp => emp.cafeId === cafeId)
        : allEmployees;
      setEmployees(filtered);
    } catch (error) {
      console.error('Failed to fetch employees:', error);
    }
  }, [cafeId]);

  // Call fetchEmployees when component loads or cafeId changes
  useEffect(() => {
    fetchEmployees();
  }, [fetchEmployees]);

  // Handle delete confirmation and API call
  const handleDelete = async () => {
    try {
      await deleteEmployee(selectedEmployee.id);
      setEmployees((prev) => prev.filter(emp => emp.id !== selectedEmployee.id));
      setConfirmDialog(false);
      showSnackbar('Employee deleted successfully', 'success');
    } catch (error) {
      console.error('Failed to delete employee:', error);
    }
  };

  // Open dialog for editing or adding a new employee
  const handleEditClick = (employee = null) => {
    setSelectedEmployee(employee || {});
    setEditDialogOpen(true);
  };

  // Handle save (after form submit), then refresh list and show success message
  const handleSave = () => {
    setEditDialogOpen(false);
    fetchEmployees();
    showSnackbar(
      selectedEmployee?.id
        ? 'Employee details updated successfully'
        : 'New employee added successfully',
      'success'
    );
  };

  // Filter employees by cafe name on search input
  const handleSearch = (value) => {
    const lowerSearch = value.toLowerCase().trim();
    const filtered = lowerSearch
      ? employees.filter(emp => emp.cafeName?.toLowerCase().includes(lowerSearch))
      : fetchEmployees(); // If cleared, reload full list
    if (Array.isArray(filtered)) {
      setEmployees(filtered);
    }
  };

  // Reusable snackbar function
  const showSnackbar = (message, severity) => {
    setSnackbar({ open: true, message, severity });
  };

  // AG Grid column definitions
  const _columnDefs = [
    { headerName: 'ID', field: 'id', width: 120 },
    { headerName: 'Name', field: 'name', width: 200 },
    { headerName: 'Email', field: 'emailAddress', width: 200 },
    { headerName: 'Phone', field: 'phoneNumber', width: 150 },
    { headerName: 'Days Worked', field: 'daysWorked', width: 150 },
    { headerName: 'Cafe Name', field: 'cafeName', width: 200 },
    {
      headerName: 'Actions',
      width: 120,
      cellRenderer: ({ data }) => (
        <Box>
          <Tooltip title="Edit">
            <IconButton size="small" onClick={() => handleEditClick(data)} sx={{ mr: 1 }}>
              <EditIcon fontSize="small" />
            </IconButton>
          </Tooltip>
          <Tooltip title="Delete">
            <IconButton
              size="small"
              color="error"
              onClick={() => {
                setSelectedEmployee(data);
                setConfirmDialog(true);
              }}
            >
              <DeleteIcon fontSize="small" />
            </IconButton>
          </Tooltip>
        </Box>
      ),
    },
  ];

  return (
    <Box>
      {/* Navigation Tabs (Cafes / Employees) */}
      <NavigationTabs />

      {/* Page Header */}
      <Typography variant="h4" gutterBottom>Employees</Typography>

      {/* Search and Add Button Row */}
      <Box display="flex" justifyContent="space-between" alignItems="center" gap={2} mb={2}>
        <TextField
          name="cafeName"
          label="Search Employees By Cafe Name"
          value={searchValue}
          onChange={(e) => {
            const value = e.target.value;
            setSearchValue(value);
            handleSearch(value);
          }}
          margin="normal"
          size="small"
        />
        <Button variant="contained" onClick={handleEditClick}>
          Add New Employee
        </Button>
      </Box>

      {/* Employee Grid */}
      <Box className="ag-theme-material" style={{ height: 400 }}>
        <AgGridReact rowData={employees} columnDefs={_columnDefs} pagination />
      </Box>

      {/* Delete Confirmation Dialog */}
      <ConfirmDialog
        open={confirmDialog}
        onClose={() => setConfirmDialog(false)}
        onConfirm={handleDelete}
        title="Delete Employee"
        message="Are you sure you want to delete this employee?"
      />

      {/* Add/Edit Employee Dialog */}
      <EmployeeDialog
        open={editDialogOpen}
        formData={selectedEmployee}
        onClose={() => setEditDialogOpen(false)}
        onSave={handleSave}
      />

      {/* Snackbar for success/error messages */}
      <Snackbar
        open={snackbar.open}
        autoHideDuration={2000}
        onClose={() => setSnackbar({ ...snackbar, open: false })}
        anchorOrigin={{ vertical: 'top', horizontal: 'right' }}
      >
        <Alert
          onClose={() => setSnackbar({ ...snackbar, open: false })}
          severity={snackbar.severity}
          sx={{ width: '100%' }}
        >
          {snackbar.message}
        </Alert>
      </Snackbar>
    </Box>
  );
}

export default EmployeePage;
