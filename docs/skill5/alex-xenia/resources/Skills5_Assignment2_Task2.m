%Parameters
Z_l = 100;
Z_0 = 50;
e_r = 2.205;
c_0 = 3e8;

f = [9.65e9 9.85e9 10e9];
Z_in = [26.5-10.4i 73.6+35.8i 39.1+29.3i];

w = 2*pi*f;
c_med = c_0/sqrt(e_r)
beta = w/c_med;

for j = 1:3
    L (j) = (1./beta(j)).*(atan(real(-Z_0*i.*((Z_l - Z_in(j))./((Z_in(j)*Z_l - Z_0^2))))))
end