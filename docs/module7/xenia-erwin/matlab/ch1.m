function z_1 = ch1(x, y, L)

N = length(y);
X = toep(x, N, L);

z_1 = inv (X' * X) * (X') * y;

end 