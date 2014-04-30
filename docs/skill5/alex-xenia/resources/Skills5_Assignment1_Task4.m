clear

%Parameters
L_0 = 0.5e-6;
C_0 = 2e-12;
Z_l = 50+50i;
Vmax = 1;
tw = 0.5e-9;
l = 1.2;
Z_0 = sqrt(L_0/C_0);
Gamma = (Z_l - Z_0)/(Z_l + Z_0);
gamma = sqrt(C_0*L_0);

%Vectors
t = 0:0.05e-9:5e-9;

t_length = length(t);

V = zeros(t_length);
I = zeros(t_length);
real_power = zeros(length(t));
imag_power = zeros(length(t));
abs_power = zeros(length(t));
eta = zeros(length(t));

U = @(t) Vmax * (heaviside(t) - heaviside(t - tw));

for j = 1:t_length
    
V(j) =          U(t(j) - gamma * l) ...
        + Gamma *   U(t(j) + gamma * (l - 2 * l)) ...
        - Gamma *   U(t(j) - gamma * (l + 2 * l)) ...
        - Gamma.^2 *U(t(j) + gamma * (l - 4 * l)) ...
        + Gamma.^2 *U(t(j) - gamma * (l + 4 * l)) ...
        - Gamma.^3 *U(t(j) - gamma * (l + 6 * l)) ...
        + Gamma.^3 *U(t(j) + gamma * (l - 6 * l));

    real_power(j) = real(V(j).^2 / Z_l);
    imag_power(j) = imag(V(j).^2 / Z_l);
    abs_power(j) = abs(V(j).^2 / Z_l);
    
    
end
plot(t, [real_power imag_power abs_power])